# Blackbird.io Lark [Beta]

Blackbird is the new automation backbone for the language technology industry. Blackbird provides enterprise-scale automation and orchestration with a simple no-code/low-code platform. Blackbird enables ambitious organizations to identify, vet and automate as many processes as possible. Not just localization workflows, but any business and IT process. This repository represents an application that is deployable on Blackbird and usable inside the workflow editor.

## Introduction

<!-- begin docs -->

Lark's comprehensive and in-depth open capabilities serve as a hub of information and an entry point for businesses, integrating with existing IT ecosystems in enterprises. It effectively supports and complements existing IT systems, enhances digital efficiency in enterprises, and assists in building an all-in-one collaborative platform, allowing employees to focus on their work and simplifying their tasks, while enabling IT personnel to develop agilely and enjoy a better user experience.


## Before setting up

Before you can connect you need to make sure that you have a Lark account  and you are setted up as an admin and the bot is enabled for the necessary groups

## Creating Lark custom application

To create a Lark custom application, follow these steps:
1. Go to the [Lark Developer Console](https://open.larksuite.com/app?lang=en-US).
1. Click on **Create Custom App**.
3. Fill in the required information:
   - **App Name**: Choose a name for your application.
   - **App Icon**: Upload an icon for your application.
   - **App Description**: Provide a brief description of your application.
   ![example1](image/README/Lark-create-app-example.png)
	
4. Click on **Create** to create your application
5. After creating the customn application, you will be redirected to the **Add features** page
   ![example2](image/README/features-example.png)

6. Select the features you want to add to your application. For current configuration, you will need **Bot**, **Web app**, **Workplace block** and **Base record view**.
7. After configuring the features you need to release the app version
   ![example3](image/README/release-version-example.png)

## Adding scopes

To add scopes to your Lark application, follow to page **Permissions & Scopes**
![example4](image/README/adding-scopes-example.png)

Click on **Add permission scopes to app** and select the scopes you want to add to your application.

For current configuration, you will need the following scopes:

**Base tables**

| Scope                  | Description                                  |
|------------------------|----------------------------------------------|
| `bitable:app`          | View, comment on, edit, and manage bases     |
| `bitable:app:readonly` | View, comment on, and export bases           |


**General scopes**

| Scope                                | Description                                    |
|--------------------------------------|------------------------------------------------|
| `contact:user.base:readonly`         | Access users’ basic information                |
| `contact:user.employee_id:readonly`  | Access users’ employee IDs; obtain user ID     |
| `drive:drive`                        | View, comment, edit, and manage all files in My Space |
| `drive:drive:readonly`               | View, comment on, and download all files in My Space |
| `drive:export:readonly`              | Export Docs documents                          |
| `drive:file`                         | Upload and download files to My Space          |
| `drive:file:readonly`                | View and download files in My Space            |


**Spreadsheets**

| Scope                         | Description                                 |
|-------------------------------|---------------------------------------------|
| `docs:document:export`        | Export document                             |
| `sheets:spreadsheet`          | View, comment, and manage Sheets            |
| `sheets:spreadsheet:readonly` | View, comment, and export Sheets            |


**Messages**

| Scope                      | Description                                      |
|----------------------------|--------------------------------------------------|
| `im:message`               | Read and send messages in private and group chats|
| `im:message:readonly`      | Read messages in private and group chats         |
| `im:message:send_as_bot`   | Send messages as an app                          |
| `im:resource`              | Read and upload images or other files            |
| `im:message:update`        | Update message                                   |


## Getting credentials

To get the Application ID and Application Secret, follow these steps:

1. Go to the [Lark Developer Console](https://open.larksuite.com/app?lang=en-US).
2. Select your application.
3. In the left sidebar, click on **Credentials and Basic info**.
4. You will find the **Application ID** and **Application Secret** in the **Credentials** section.
   ![example5](image/README/credentials-example.png)


## Connecting

1. Navigate to Apps, and identify the **Lark** app. You can use search to find it.
2. Click _Add Connection_.
3. Name your connection for future reference e.g. 'My organization'.
4. Input the 'Application ID' and 'Application Secret' from your Lark application.
5. Click _Authorize connection_.

## Actions

### Messages

- **Send a message** sends a message to a Lark channel or directly to a user
- **Send file** sends a file to a Lark channel or directly to a user
- **Get message** gets a message by it ID
- **Edit message** edits a message by it ID

### User

- **Get user information from email** gets user information from an email address
- **Get user information** gets user information by user ID

### Base table

- **Search base tables** searches for base tables
- **Get base record** gets a base record
- **Update base record** updates a base record 
- **Get person entry from base table record** gets a person entry from a base table record
- **Get date entries from base table record** gets date entries from a base table record
- **Download attachments from base table record** downloads attachments from a base table record
- **Insert row to base table** inserts a row to a base table

### Spreadsheets

- **Create spreadsheet** creates a new spreadsheet
- **Find cells** finds cells in a spreadsheet by query
- **Add rows or columns** adds empty rows or columns to a spreadsheet
- **Delete rows or columns** deletes rows or columns from a spreadsheet
- **Insert rows** inserts rows into a spreadsheet. If there are existing rows, the new rows will be inserted before the existing rows
- **Add or update rows/columns** adds or updates rows or columns in a spreadsheet.  If there are existing rows/columns, the new rows/columns will rewrite the existing one. By default uses delimeter `,` to split the values.
- **Get range cells values** gets the values of a range of cells in a spreadsheet by specified range
- **Get sheet cell** retrieves value for a specified cell in a spreadsheet
- **Update sheet cell** updates value for a specified cell in a spreadsheet

## Events

- **On message received** triggers when a message is received in a Lark channel or directly to a user
- **On reaction added** triggers when a reaction is added to a message in a Lark channel or directly to a user
- **On user added to group** triggers when a user is added to a group
- **On file edited** triggers when a file is edited in a Lark channel or directly to a user
- **On new rows added** triggers when new rows are added to a spreadsheet
- **On base table new rows added** triggers when new rows are added to a base table


## Feedback

Do you want to use this app or do you have feedback on our implementation? Reach out to us using the [established channels](https://www.blackbird.io/) or create an issue.

<!-- end docs -->
