
# Get current path

$CurrentPath = (Get-Location -PSProvider FileSystem).ProviderPath

# Recursively deletes all 'pdb' files inside current folder

$FilesToRemove = Get-ChildItem -Rec | Where {$_.Extension -match "pdb"} 

# remove files and print to output
if($FilesToRemove -ne $null)
{			
    Write-Host 
	foreach ($item in $FilesToRemove) 
	{ 		
		remove-item $item.FullName -Force -Recurse;
		Write-Host "Removed: ." -nonewline; 
		Write-Host $item.FullName.replace($CurrentPath, ""); 
	} 
}

# Recursively deletes all 'debug', 'obj', '.git', '.vs' (or any other specified) folders inside current folder

# recursively get all folders matching given includes, except ignored folders
$FoldersToRemove = Get-ChildItem .\ -include debug,obj,.git,.vs -Recurse   | where {$_ -notmatch '_tools' -and $_ -notmatch '_build'} | foreach {$_.fullname}

# recursively get all folders matching given includes
$AllFolders = Get-ChildItem .\ -include debug,obj,.git,.vs -Recurse | foreach {$_.fullname}

# subtract arrays to calculate ignored ones
$IgnoredFolders = $AllFolders | where {$FoldersToRemove -notcontains $_} 

# remove folders and print to output
if($FoldersToRemove -ne $null)
{			
    Write-Host 
	foreach ($item in $FoldersToRemove) 
	{ 
		remove-item $item -Force -Recurse;
		Write-Host "Removed: ." -nonewline; 
		Write-Host $item.replace($CurrentPath, ""); 
	} 
}

# print ignored folders	to output
if($IgnoredFolders -ne $null)
{
    Write-Host 
	foreach ($item in $IgnoredFolders) 
	{ 
		Write-Host "Ignored: ." -nonewline; 
		Write-Host $item.replace($CurrentPath, ""); 
	} 
	
	Write-Host 
	Write-Host $IgnoredFolders.count "folders ignored" -foregroundcolor yellow
}

# print summary of the operation
Write-Host 
if($FoldersToRemove -ne $null)
{
	Write-Host $FoldersToRemove.count "folders removed" -foregroundcolor green
}
else { 	Write-Host "No folders to remove" -foregroundcolor green }	

# print summary of the operation
Write-Host 
if($FilesToRemove -ne $null)
{
	Write-Host $FilesToRemove.count "files removed" -foregroundcolor green
}
else { 	Write-Host "No files to remove" -foregroundcolor green }	


Write-Host 

# prevent closing the window immediately
$dummy = Read-Host "Completed, press enter to continue."