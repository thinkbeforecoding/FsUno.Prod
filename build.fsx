#r "packages/Fake/tools/FakeLib.dll"
#r "System.Xml.Linq"

open System
open System.Xml.Linq
open Fake   
open NuGetHelper
open Fake.Git

let title = "FsUno.Prod"
let authors = [ "Jérémie Chassaing" ]
let githubLink = "https://github.com/thinkbeforecoding/FsUno.Prod/"

let projns = XNamespace.op_Implicit "http://schemas.microsoft.com/developer/msbuild/2003"
let xn = XName.op_Implicit

let journeyOutDir = "bin\Journey"

Target "Clean" <| fun _ ->
    CleanDir "bin"

Target "Journey" <| fun _ ->
    let template = @"FsUno.Journey\Template\template-project.html"

    let journeyProject = @"FsUno.Journey\FsUno.Journey.fsproj"

    let index =
        let project = loadProject journeyProject
        project.Descendants(projns + "None").Attributes(xn "Include")
        |> Seq.map (fun attr -> attr.Value)
        |> Seq.filter (fun f -> ext f = ".fsx")
        |> Seq.map (fun file -> 
            let name = fileNameWithoutExt file
            let uri = Uri.EscapeUriString name + ".html"
            name, uri)
        |> Seq.skip 1
        |> Seq.toList

    let tableOfContent =
        ("The Journey", "index.html" ) :: index
        |> List.map (fun (name, uri) -> sprintf """<li><a href="%s">%s</a></li>""" uri name )
        |> separated "\n            "

    let projInfo = 
       [ "page-description", title
         "page-author", separated ", " authors  
         "project-author", separated ", " authors 
         "github-link", githubLink 
         "project-github", "http://github.com/fsharp/fake" 
         "root", "http://thinkbeforecoding.github.io/FsUno.Prod" 
         "project-name", title 
         "table-of-content", tableOfContent] 

    FSharpFormatting.CreateDocs "FsUno.Journey" journeyOutDir template projInfo

    let sourceDir = directoryInfo @"tools\FSharp.Formatting.CommandTool\styles\"
    let outDir = directoryInfo (journeyOutDir @@ "content")
    copyRecursive sourceDir outDir true
    |> ignore

Target "ReleaseJourney" <| fun _ -> 
    let pages = "bin/gh-pages"
    CleanDir pages
    cloneSingleBranch "" "git@github.com:thinkbeforecoding/FsUno.Prod.git" "gh-pages" pages
 
    fullclean pages
    CopyRecursive journeyOutDir pages true |> printfn "%A" 
    StageAll pages 
    Commit pages "Update generated documentation" 
    Branches.push pages 
 


"Clean" ==> "Journey" ==> "ReleaseJourney"

RunTargetOrDefault "Journey"
