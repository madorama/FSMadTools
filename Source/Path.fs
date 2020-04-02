namespace FSMadTools

module Path =
  open UnityEditor

  let dllPath =
    System.Reflection.Assembly.GetExecutingAssembly().Location
    |> (fun x -> x.Replace("\\", "/"))

  let dllDirectory =
    let p = dllPath |> System.IO.Path.GetDirectoryName |> (fun x -> x.Replace("\\", "/"))
    p + "/"

  let dllProjectRelativeDirectory =
    dllDirectory
    |> FileUtil.GetProjectRelativePath
