Write-Output "Compilation de SimWinO en mode Release..."

Push-Location -Path ".."
dotnet publish -c Release -r win-x64 --no-self-contained -o SimWinO.Installer\build --nologo -v q
Pop-Location

# Hash generation
$HashFile = "version.sha256"
$HashPath = -join("build\", $HashFile)
Set-Content -Path $HashPath -Value $(Get-FileHash -Path "build\SimWinO.WPF.exe").Hash

Write-Output "Compilation et compression de l'installeur..."

.\innosetup\ISCC.exe setup.iss /Q

Write-Output "Envoi de la nouvelle version sur le serveur de publication..."

# Server
$Server = "simwino.rousseur.fr"

# Authentication
$Username = "simwino"

# Paths
$LocalFile = ".\Output\SimWinO_setup.exe"
$RemoteFile = "~/www/SimWinO_setup.exe"
$RemoteHash = (-join("~/www/", $HashFile))

Icacls ssh_key /q /Inheritance:r | Out-Null
Icacls ssh_key /q /Grant:r ${env:Username}:"(R)" | Out-Null
scp -i ssh_key -o "StrictHostKeyChecking accept-new" $LocalFile ${Username}@${Server}:${RemoteFile}
scp -i ssh_key -o "StrictHostKeyChecking accept-new" $HashPath ${Username}@${Server}:${RemoteHash}
