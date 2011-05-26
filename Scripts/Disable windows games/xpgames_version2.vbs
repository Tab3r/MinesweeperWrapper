' Uninstall games from Windows XP 
'============================================================================= 
Dim Fso : Set Fso = CreateObject("Scripting.FileSystemObject") 
Set WshShell = WScript.CreateObject("WScript.Shell")
sPrograms = WshShell.SpecialFolders("AllUsersPrograms") 
If (Fso.FolderExists(sPrograms & "\Games")) Then 
' Create file for uninstalling games 
Set f = Fso.CreateTextFile("c:\windows\inf\wmdtocm.txt", True) 
f.WriteLine("[Components]")
f.WriteLine("freecell=off")
f.WriteLine("hearts=off") 
f.WriteLine("minesweeper=off") 
f.WriteLine("msnexplr=off") 
f.WriteLine("pinball=off") 
f.WriteLine("solitaire=off") 
f.WriteLine("spider=off") 
f.WriteLine("zonegames=off") 
f.Close 
WshShell.Run "sysocmgr.exe /i:c:\windows\inf\sysoc.inf /u:""c:\windows\inf\wmdtocm.txt"" /q", 1, True 
Fso.DeleteFolder(sPrograms & "\Games"), True 
End If