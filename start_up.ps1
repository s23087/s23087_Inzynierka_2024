cd "C:\dev\s23087_Inzynierka_2024"
$newWindowCommand = @"
cd "web\handler_b2b"
npm run build
npm run start
"@

$tempScriptFile = [System.IO.Path]::GetTempFileName() + ".ps1"
Set-Content -Path $tempScriptFile -Value $newWindowCommand

Start-Process powershell.exe -ArgumentList "-NoExit", "-File", $tempScriptFile

cd database_comunicator
dotnet run --launch-profile "production"