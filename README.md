# JiraFlow
> Synchronise your Workflowy notes to Jira.

## Usage

Currently we are not publishing JiraFlow via any mechanism. To run JiraFlow you will need to clone this repository and run the code directly.

1. Clone Repository

```sh
git clone git@github.com:uatec/jiraflow
```

2. Install .Net Core

JiraFlow is written in .Net Core. To build from source you need to download and install the [.Net Core SDK 2.2](https://dotnet.microsoft.com/download).

3. Configure JiraFlow

Create a file in the root called `appsettings.json`.

```json
{
    "defaultEpicLink": "EG-123", // The Jira item number of the epic to assign all tasks to
    "host": "https://somejira.net", // The hostname of your Jira Server
    "username": "someuser", // Your Jira username
    "password": "my-password", // Your Jira password
    "workflowytoken": "abcd1234abcd1234", // Sign in to workflow in your browser and get the value of the cookie called 'sessionid'
    "projectCode": "EG", // The code for the project to add all items to 
    "assignee": "someuser", // The username of the reporter and assignee in Jira
    "sync_interval": 30000 // Time in milliseconds to synchronise changes
}
```

## Features

- Create new Jira items from Workflowy notes.
 - Add a `#nojira` tag to workflowy items and they will be created in Jira
 - Bullet titles will be used for the Jira Summary
 - Bullet notes will be used for the Jira Description



## Meta
Robert Stiff – [@uatecuk](https://twitter.com/UatecUK) – uatecuk@gmail.com

Distributed under the MIT license. See ``LICENSE`` for more information.

https://github.com/uatec/jiraflow

## Contributing

1. Fork it (https://github.com/uatec/jiraflow/fork)
2. Create your feature branch (git checkout -b feature/fooBar)
3. Commit your changes (git commit -am 'Add some fooBar')
4. Push to the branch (git push origin feature/fooBar)
5. Create a new Pull Request