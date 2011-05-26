Set FS = CreateObject("Scripting.FileSystemObject")
set ws = CreateObject("WScript.Shell")
set objshell = createobject("shell.application")
Set WshNetwork = CreateObject("WScript.Network")
On error resume next

prog1 = "sysocmgr /i:c:\windows\inf\sysoc.inf /u:c:\radiadata\xpgames.txt /q"

ws.run prog1
fs.deletefile("%systemroot%\system32\dllcache\sol.exe")
fs.deletefile("%systemroot%\system32\dllcache\winmine.exe")
fs.deletefile("%systemroot%\system32\dllcache\freecell.exe")
fs.deletefile("%systemroot%\system32\dllcache\pinball.exe")
fs.deletefile("%systemroot%\system32\dllcache\spider.exe")
fs.deletefile("%systemroot%\system32\dllcache\mshearts.exe")
fs.deletefolder("C:\Documents and Settings\All Users\Start Menu\Programs\Games")