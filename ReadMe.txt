Pre Setup to run the solution:
----------------------------------
All the required settings has been made configurable and added in <appsettings> section of the project's config file.

Make sure below folders are present before running:

1. Input folder: C:\Temp 
   This is the root folder to place the input zip file provided in the requirements "aatdeveloperrecruitmentcodeexercisesecofficialsensi.zip"
   
2. Extracted Folder: C:\Temp\Extracted
   Root folder where all the individual extracted folders reside
   
3. XML Validation Folder: C:\Temp\XmlSchemaValidate
   This folder is to temporarily extract party XML and XSD to validate the XML.

4. Logs Folder: C:\Temp\ZipUtilityLogs

SMTP settings:

I have used gmail SMTP to send the notification to admin user.
Make sure the below app settings are updated with correct values.

1. SMTP.CredentialUser
2. SMTP.CredentialPassword
3. Notification.AdminEmailAddress