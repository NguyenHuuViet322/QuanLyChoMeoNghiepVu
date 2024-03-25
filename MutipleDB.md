### PMC
	1.  Add-Migration
	Add-Migration [-Name] <String> [-OutputDir <String>] [-Context <String>] [-Project <String>] [-StartupProject <String>] [<CommonParameters>]
	Eg: Add-Migration -Name initDBCache -OutputDir "Migrations/DasCache" -Context DASCacheContext 
	2.	Update-Database
	Update-Database [[-Migration] <String>] [-Context <String>] [-Project <String>] [-StartupProject <String>] [<CommonParameters>]
	Eg: Update-Database -Migration initDBCache -Context DASCacheContext		
	3. Drop-Database  
	Drop-Database [-Context <String>] [-Project <String>] [-StartupProject <String>] [-WhatIf] [-Confirm] [<CommonParameters>]
	Eg: Drop-Database -Context DASCacheContext