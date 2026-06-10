# GitHubActivityTracker
A C#-based CLI Task Tracker demonstrating object-oriented programming (OOP) principles, efficient data storage, and structured workflow management.

## 🚀 Features

* Live API Integration: It consumes endpoint from Git API using HTTPPCLIENT.
*Interactive Event Workflow: User can view the fetched data directly if the status code is success. He can also persist or dispose the data.
*Immutable Local Storage: The persisted data is immutable, preserving the original data as fetched.
*Custom JSON ParsingP: The data is read as string, deserialized to JSON ObjectElement and parsed into C# strongly-typed C# object.

### 🏗️ Architecture and Concept Applied
*   Layered Architecture: Each layer is separated into infrastructure, application and entity layer.
*   Modular Extraction Pattern: Features dedicated to extracting and parsed JSON ObjectElement into strongly-typed C# objects. to          ensure single responsibility principle(SRP)
* Asynchronous Networking : utilizes Async/Await paradigm with HTTPCLIENT for non-blocked network request

####  🔗🖇️LINKS
*git url
Project URL: https://github.com/Godsbelovedev/GitHubActivityTracker

*road map url
Project URL: https://roadmap.sh/projects/github-user-activity
