using Apps.Appname.Api;
using Apps.Lark.Models.Request;
using Apps.Lark.Models.Response;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace Apps.Appname.Actions;

[ActionList]
public class MessageActions(InvocationContext invocationContext, IFileManagementClient fileManagementClient) : Invocable(invocationContext)
{
    private IFileManagementClient FileManagementClient { get; set; } = fileManagementClient;

    [Action("Send message", Description = "Send message")]
    public async Task<SendMessageResult> SendMessage([ActionParameter] SendMessageRequest input)
    {
        if (string.IsNullOrWhiteSpace(input.ChatsId) && string.IsNullOrWhiteSpace(input.UserId))
        {
            throw new PluginMisconfigurationException("Either Chat Id or User Id must be provided. Please check the input");
        }

        if (!string.IsNullOrWhiteSpace(input.ChatsId) && !string.IsNullOrWhiteSpace(input.UserId))
        {
            throw new PluginMisconfigurationException("Only one of Chat Id or User Id must be provided, not both. Please check the input");
        }

        string receiveId, receiveIdType;
        if (!string.IsNullOrWhiteSpace(input.ChatsId))
        {
            receiveId = input.ChatsId;
            receiveIdType = "chat_id";
        }
        else
        {
            receiveId = input.UserId;
            receiveIdType = "user_id";
        }

        var larkClient = new LarkClient(invocationContext.AuthenticationCredentialsProviders);
        var request = new RestRequest("/im/v1/messages", Method.Post);

        request.AddQueryParameter("receive_id_type", receiveIdType);

        var contentJson = JsonConvert.SerializeObject(new { text = input.MessageText });

        request.AddJsonBody(new
        {
            receive_id = receiveId,
            msg_type = "text",
            content = contentJson
        });

        var response = await larkClient.ExecuteWithErrorHandling<SendMessageResponse>(request);
        return new SendMessageResult
        {
            MessageId = response.Data.MessageId,
            ChatId = response.Data.ChatId,
            Content = response.Data.Body.Content,
            CreateTime = response.Data.CreateTimeUtc,
            UpdateTime = response.Data.UpdateTimeUtc,
            Sender = response.Data.Sender
        };
    }

    [Action("Send file", Description = "Send file message")]
    public async Task<SendMessageResult> SendFile([ActionParameter] SendFileRequest input)
    {
        if (string.IsNullOrWhiteSpace(input.ChatsId) && string.IsNullOrWhiteSpace(input.UserId))
        {
            throw new PluginMisconfigurationException("Either Chat Id or User Id must be provided. Please check the input");
        }
        if (!string.IsNullOrWhiteSpace(input.ChatsId) && !string.IsNullOrWhiteSpace(input.UserId))
        {
            throw new PluginMisconfigurationException("Only one of Chat Id or User Id must be provided, not both. Please check the input");
        }

        var fileName = string.IsNullOrWhiteSpace(input.FileName)
       ? input.FileContent.Name
       : input.FileName;

        string receiveId, receiveIdType;
        if (!string.IsNullOrWhiteSpace(input.ChatsId))
        {
            receiveId = input.ChatsId;
            receiveIdType = "chat_id";
        }
        else
        {
            receiveId = input.UserId;
            receiveIdType = "user_id";
        }

        var larkClient = new LarkClient(invocationContext.AuthenticationCredentialsProviders);

        await using var fileStream = await FileManagementClient.DownloadAsync(input.FileContent);
        using var memoryStream = new MemoryStream();
        await fileStream.CopyToAsync(memoryStream);
        memoryStream.Position = 0;

        string uploadFileType, messageType;
        if (input.FileContent.ContentType.StartsWith("image/"))
        {
            uploadFileType = "image";
            messageType = "image";
        }
        else if (input.FileContent.ContentType.StartsWith("video/"))
        {
            uploadFileType = "mp4";
            messageType = "video";
        }
        else if (input.FileContent.ContentType.StartsWith("audio/"))
        {
            uploadFileType = "opus";
            messageType = "audio";
        }
        else if (input.FileContent.ContentType == "application/pdf")
        {
            uploadFileType = "pdf";
            messageType = "file";
        }
        else if (input.FileContent.ContentType == "application/msword" ||
                 input.FileContent.ContentType.Contains("wordprocessingml.document"))
        {
            uploadFileType = "doc";
            messageType = "file";
        }
        else if (input.FileContent.ContentType == "application/vnd.ms-excel" ||
                 input.FileContent.ContentType.Contains("spreadsheetml.sheet"))
        {
            uploadFileType = "xls";
            messageType = "file";
        }
        else if (input.FileContent.ContentType == "application/vnd.ms-powerpoint" ||
                 input.FileContent.ContentType.Contains("presentationml.presentation"))
        {
            uploadFileType = "ppt";
            messageType = "file";
        }
        else
        {
            uploadFileType = "stream";
            messageType = "file";
        }

        var fileBytes = memoryStream.ToArray();
        var uploadRequest = new RestRequest("/im/v1/files", Method.Post){AlwaysMultipartFormData = true};
        //uploadRequest.AddHeader("Content-Type", "multipart/form-data");
        uploadRequest.AddParameter("file_type", uploadFileType);
        uploadRequest.AddParameter("file_name", fileName);
        uploadRequest.AddFile("file", fileBytes, input.FileContent.Name, input.FileContent.ContentType);

        var uploadResponse = await larkClient.ExecuteWithErrorHandling<UploadFileResponse>(uploadRequest);
        var fileKey = uploadResponse.Data.FileKey;

        string contentJson = messageType switch
        {
            "image" => JsonConvert.SerializeObject(new { image_key = fileKey }),
            "video" => JsonConvert.SerializeObject(new { video_key = fileKey }),
            "audio" => JsonConvert.SerializeObject(new { audio_key = fileKey }),
            _ => JsonConvert.SerializeObject(new { file_key = fileKey })
        };

        var messageRequest = new RestRequest("/im/v1/messages", Method.Post);
        messageRequest.AddQueryParameter("receive_id_type", receiveIdType);
        messageRequest.AddJsonBody(new
        {
            receive_id = receiveId,
            msg_type = messageType,
            content = contentJson
        });

        var messageResponse = await larkClient.ExecuteWithErrorHandling<SendMessageResponse>(messageRequest);
        return new SendMessageResult
        {
            MessageId = messageResponse.Data.MessageId,
            ChatId = messageResponse.Data.ChatId,
            Content = messageResponse.Data.Body.Content,        
            CreateTime = messageResponse.Data.CreateTimeUtc,
            UpdateTime = messageResponse.Data.UpdateTimeUtc,
            Sender = messageResponse.Data.Sender
        };

    }

    [Action("Get message", Description = "Get message by message_id")]
    public async Task<GetMessageResult> GetMessage([ActionParameter] GetMessageRequest input)
    {
        var larkClient = new LarkClient(invocationContext.AuthenticationCredentialsProviders);

        var request = new RestRequest($"/im/v1/messages/{input.MessageId}", Method.Get);

        var response = await larkClient.ExecuteWithErrorHandling<GetMessageResponse>(request);
        var item = response.Data.Items?.FirstOrDefault()
                        ?? throw new PluginApplicationException("Message not found.");

        return new GetMessageResult
        {
            Body = item.Body,
            ChatId = item.ChatId,
            CreateTime = item.CreateTimeUtc,
            MessageId = item.MessageId,
            Sender = item.Sender
        };
    }

    [Action("Edit message", Description = "Edit an existing message")]
    public async Task<SendMessageResult> EditMessage([ActionParameter] EditMessageRequest input)
    {
        var larkClient = new LarkClient(invocationContext.AuthenticationCredentialsProviders);

        var request = new RestRequest($"/im/v1/messages/{input.MessageId}", Method.Put);


        var contentJson = JsonConvert.SerializeObject(new { text = input.MessageText });

        request.AddJsonBody(new
        {
            msg_type = "text",
            content = contentJson
        });

        var response = await larkClient.ExecuteWithErrorHandling<SendMessageResponse>(request);
        return new SendMessageResult
        {
            MessageId = response.Data.MessageId,
            ChatId = response.Data.ChatId,
            Content = response.Data.Body.Content,
            CreateTime = response.Data.CreateTimeUtc,
            UpdateTime = response.Data.UpdateTimeUtc,
            Sender = response.Data.Sender
        };
    }

}