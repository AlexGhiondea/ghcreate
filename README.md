# GHCreate
Tool that will check/list/create objects in GitHub repositories

![Build And Test](https://github.com/AlexGhiondea/ghcreate/workflows/Build%20And%20Test/badge.svg)

You can specify multiple objects that should be created on GitHub. Currently this supports creating Labels and Milestones.

# Usage

You can use the `ghcreate` tool to create labels or milestones in any number of GitHub repositories.

To get started, you can look at the short help for the tool:

```
Usage:
 GHCreator.exe  Create objectsFile repos [-token value]
 GHCreator.exe  CreateOrUpdate objectsFile repos [-token value]
 GHCreator.exe  List repos [-token value]
 GHCreator.exe  Check objectsFile repos [-token value]
```

If you want to know more about a particular command, you can do that too!

```
Usage:
 GHCreator.exe Create objectsFile repos [-token value]
  - objectsFile : The file containing the list of objects to create or check. The structure is: <type>,<name>,<description>,[any other type specific information] (string, required)
  - repos       : The list of repositories where to add the milestones to. The format is: owner\repoName;owner\repoName (string, required)
  - token       : The GitHub authentication token. (string, default=)

 GHCreator.exe CreateOrUpdate objectsFile repos [-token value]
  - objectsFile : The file containing the list of objects to create or check. The structure is: <type>,<name>,<description>,[any other type specific information] (string, required)
  - repos       : The list of repositories where to add the milestones to. The format is: owner\repoName;owner\repoName (string, required)
  - token       : The GitHub authentication token. (string, default=)

 GHCreator.exe List repos [-token value]
  - repos : The list of repositories where to add the milestones to. The format is: owner\repoName;owner\repoName (string, required)
  - token : The GitHub authentication token. (string, default=)

 GHCreator.exe Check objectsFile repos [-token value]
  - objectsFile : The file containing the list of objects to create or check. The structure is: <type>,<name>,<description>,[any other type specific information] (string, required)
  - repos       : The list of repositories where to add the milestones to. The format is: owner\repoName;owner\repoName (string, required)
  - token       : The GitHub authentication token. (string, default=)
```

# Input file format

The tool requires a file that describes the types of objects that need to be created. At this time, it can only create Labels and Milestones. 

The format of the file is:
```
<TypeOfObject>,<Title>,<Description>,[<additionalData1>,...,<additionalDatan>]
```

## Example of input file

For example, here is a file that will ensure that the labels in GitHub repos follow the Azure-SDK guidelines

```
Label,Client,Issues that are going to be addressed by a change in the data-plane client library,ffeb77
Label,Mgmt,Issues that are going to be addressed by a change in the management client library,ffeb77
Label,EngSys,Engineering System,ffeb77
Label,Service,The fix for the issue needs to go in the service,ffeb77
Label,Service Attention,The issue requires the service team to investigate further.,10066b
Label,needs-attention,Issue that needs attention from the SDK team,3BA0F8
Label,needs-triage,This is a new issue that needs to be triaged to the appropriate team,ededed
Label,needs-team-triage,Issue that needs the team to triage the issue.,ededed
Label,needs-author-feedback,Issue needs more information for the SDK team to take action on it,f72598
Label,bug,This issue requires a change to an existing behavior in the product in order to resolve.,eaa875
Label,feature-request,This issue requires a new behavior in the product in order to resolve.,eaa875
Label,question,The issue doesn't require a change to the product in order to be resolved. Most issues start as that,eaa875
Label,no-recent-activity,There has been no recent activity on this issue,bbbbbb
```
