param (
# the directoy to package, should be the output directory of the plugin class library
    [string]$targetDirectory,

# Msix Name
    [string]$targetName,

    [string]$outputDir,
# the full path to the certificate used for msix package signing
    [string]$certificatePath,

# the password to use for msix package signing
    [string]$certificatePassword=""
)

$packagePath = Join-Path -Path $targetDirectory -ChildPath $targetName

# delete the old .msix package file if it exists
Remove-Item $packagePath -ErrorAction Ignore
 
# 打包
$makeAppxResult = Start-Process -FilePath "makeappx.exe" -ArgumentList @("pack", "/o", "/d", $outputDir, "/p", $packagePath) -NoNewWindow -Wait -PassThru

if ($makeAppxResult.ExitCode -ne 0) {
    Write-Error "MakeAppx 打包失败，退出码 $($makeAppxResult.ExitCode)"
    exit $makeAppxResult.ExitCode
}

$signArgs = @(
    "sign",
    "/fd", "SHA256",
    "/a",
    "/f", $certificatePath
)

if (-not [string]::IsNullOrEmpty($certificatePassword)) {
    $signArgs += @("/p", $certificatePassword)
}

$signArgs += $packagePath

$signResult = Start-Process -FilePath "signtool.exe" -ArgumentList $signArgs -NoNewWindow -Wait -PassThru

if ($signResult.ExitCode -ne 0) {
    Write-Error "签名失败，退出码 $($signResult.ExitCode)"
    exit $signResult.ExitCode
}

# 成功输出
Write-Output "Package Msix Successed: $packagePath"

