param (
    [Parameter(Mandatory = $true)]
    [string]$PackageFamilyGuid
)

# Check for loopback exemption
$output = CheckNetIsolation.exe LoopbackExempt -s

if ($output -match $PackageFamilyGuid) {
    Write-Output "The package '$PackageFamilyGuid' already has a loopback exemption."
} else {
    Write-Output "The package '$PackageFamilyGuid' does NOT have a loopback exemption. Adding it now..."
    CheckNetIsolation.exe LoopbackExempt -a -n=$PackageFamilyGuid

    # Verify if it was successfully added
    $outputAfterAdd = CheckNetIsolation.exe LoopbackExempt -s
    if ($outputAfterAdd -match $PackageFamilyGuid) {
        Write-Output "Loopback exemption successfully added for '$PackageFamilyGuid'."
    } else {
        Write-Output "Failed to add loopback exemption for '$PackageFamilyGuid'."
    }
}
