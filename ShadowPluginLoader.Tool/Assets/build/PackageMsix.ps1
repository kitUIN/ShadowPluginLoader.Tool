param (
# the directoy to package, should be the output directory of the plugin class library
    [string]$targetDirectory,

# Msix Name
    [string]$targetName,

# the full path to the certificate used for msix package signing
    [string]$certificatePath,

# the password to use for msix package signing
    [string]$certificatePassword
)

$packagePath = Join-Path -Path $targetDirectory -ChildPath $targetName

# delete the old .msix package file if it exists
Remove-Item $packagePath -ErrorAction Ignore

#create a signed package from the target directory
makeappx pack /o /d $targetDirectory /p $packagePath
signtool sign /fd SHA256 /a /f $certificatePath /p $certificatePassword $packagePath
