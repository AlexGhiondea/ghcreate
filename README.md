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
 GHCreator.exe  List type repos [-token value]
 GHCreator.exe  Check objectsFile repos [-token value]
```

If you want to know more about a particular command, you can do that too!

```
Usage:
 GHCreator.exe Create objectsFile repos [-token value]
  - objectsFile : The file containing the list of objects to create or check. The structure is: <type>,<name>,<description>,[any other type specific information] (string, required)
  - repos       : The list of repositories where to add the milestones to. The format is: owner\repoName (list, required)
  - token       : The GitHub authentication token. (string, default=)

 GHCreator.exe CreateOrUpdate objectsFile repos [-token value]
  - objectsFile : The file containing the list of objects to create or check. The structure is: <type>,<name>,<description>,[any other type specific information] (string, required)
  - repos       : The list of repositories where to add the milestones to. The format is: owner\repoName (list, required)
  - token       : The GitHub authentication token. (string, default=)

 GHCreator.exe List type repos [-token value]
  - type  : The type of object we want to list (one of Milestone,Label, required)
  - repos : The list of repositories where to add the milestones to. The format is: owner\repoName (list, required)
  - token : The GitHub authentication token. (string, default=)

 GHCreator.exe Check objectsFile repos [-token value]
  - objectsFile : The file containing the list of objects to create or check. The structure is: <type>,<name>,<description>,[any other type specific information] (string, required)
  - repos       : The list of repositories where to add the milestones to. The format is: owner\repoName (list, required)
  - token       : The GitHub authentication token. (string, default=)
```

# Input file format

The tool requires a file that describes the types of objects that need to be created. At this time, it can only create Labels and Milestones. 

The format of the file is:
```
<TypeOfObject>,<Title>,<Description>,[<additionalData1>,...,<additionalDatan>]
```

## Example of input file

For example, here is a file that will ensure that the labels in GitHub repos follow the Azure-SDK guidelines.

As you can see, you can specify multiple types of objects in the same input file.

```
Label,Client,This issue points to a problem in the data-plane of the library.,ffeb77
Label,Mgmt,This issue points to a problem in the management-plane of the library.,ffeb77
Label,EngSys,This issue is responsible by the Engineering System team.,ffeb77
Label,Service,This issue points to a problem in the service.,ffeb77
Label,Service Attention,This issue requires the service team to investigate further and is responsible by the service team.,10066b
Label,needs-attention,This issue needs attention from the service team or the SDK team.,3BA0F8
Label,needs-triage,This is a new issue that needs to be triaged to the appropriate team.,ededed
Label,needs-team-triage,This issue needs the team to triage.,ededed
Label,needs-author-feedback,More information is needed from author to address the issue.,f72598
Label,bug,This issue requires a change to an existing behavior in the product in order to be resolved.,eaa875
Label,feature-request,This issue requires a new behavior in the product in order be resolved.,eaa875
Label,question,This issue doesn't require a change to the product in order to be resolved. Most issues start as this.,eaa875
Label,no-recent-activity,There has been no recent activity on this issue.,bbbbbb
Milestone,[2020] June,[2020] June,6/5/2020
```
