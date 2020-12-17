[Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12

# Install NuGet provider if needed
if ((Get-PackageProvider -Name NuGet).version -lt 2.8.5.201 ) {
    try {
        Install-PackageProvider -Name NuGet -MinimumVersion 2.8.5.201 -Confirm:$False -Force -Scope CurrentUser | Out-Null
    }
    catch [Exception] {
        $_.message 
        exit
    }
}

# Import or install module as needed
function Get-ExternalModule($m) {
    # If module is imported say that and do nothing
    if (Get-Module | Where-Object {$_.Name -eq $m}) {
        return
    }
    else {
        # If module is not imported, but available on disk then import
        if (Get-Module -ListAvailable | Where-Object {$_.Name -eq $m}) {
            Import-Module $m | Out-Null
        }
        else {
            # If module is not imported, not available on disk, but is in online gallery then install and import
            if (Find-Module -Name $m | Where-Object {$_.Name -eq $m}) {
                Install-Module -Name $m -Force -Scope CurrentUser | Out-Null
                Import-Module $m | Out-Null
            }
            else {
                # If module is not imported, not available and not in online gallery then abort
                Write-Host "Le module $m n'a pas été trouvé dans la gallerie locale ou la gallerie en ligne."
                exit 1
            }
        }
    }
}

# Import needed modules
Get-ExternalModule 7Zip4PowerShell

# Install InnoSetup if needed
if (!(Test-Path -PathType Container -Path "innosetup")) {
    $url = "https://github.com/portapps/innosetup-portable/releases/download/6.1.2-4/innosetup-portable-win32-6.1.2-4.7z"
    $7z = "${PSScriptRoot}\innosetup.7z"

    Write-Output "Téléchargement de InnoSetup en mode portable..."

    (New-Object System.Net.WebClient).DownloadFile($url, $7z);

    Write-Output "Extraction de InnoSetup..."

    Expand-7Zip -ArchiveFileName $7z -TargetPath "${PSScriptRoot}\innosetup"

    Write-Output "Nettoyage..."

    Remove-Item -Path $7z

    Write-Output "InnoSetup est prêt à l'emploi."
}

# Building the project
Write-Output "Compilation de SimWinO en mode Release..."

Push-Location -Path ".."
dotnet publish -c Release -r win-x64 --no-self-contained -o SimWinO.Installer\build --nologo -v q
Pop-Location

# Hash generation
$HashFile = "version.sha256"
$HashPath = -join("build\", $HashFile)
Set-Content -Path $HashPath -Value $(Get-FileHash -Path "build\SimWinO.WPF.exe").Hash

Write-Output "Compilation et compression de l'installeur..."

.\innosetup\app\ISCC.exe setup.iss /Q

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
