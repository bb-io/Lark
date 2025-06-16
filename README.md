# Blackbird.io Lark [Beta]

Blackbird is the new automation backbone for the language technology industry. Blackbird provides enterprise-scale automation and orchestration with a simple no-code/low-code platform. Blackbird enables ambitious organizations to identify, vet and automate as many processes as possible. Not just localization workflows, but any business and IT process. This repository represents an application that is deployable on Blackbird and usable inside the workflow editor.

## Introduction

<!-- begin docs -->

Lark's comprehensive and in-depth open capabilities serve as a hub of information and an entry point for businesses, integrating with existing IT ecosystems in enterprises. It effectively supports and complements existing IT systems, enhances digital efficiency in enterprises, and assists in building an all-in-one collaborative platform, allowing employees to focus on their work and simplifying their tasks, while enabling IT personnel to develop agilely and enjoy a better user experience.


## Before setting up

Before you can connect you need to make sure that you have a Lark account  and you are setted up as an admin and the bot is enabled for the necessary groups

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


## Feedback

Do you want to use this app or do you have feedback on our implementation? Reach out to us using the [established channels](https://www.blackbird.io/) or create an issue.

<!-- end docs -->
