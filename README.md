# 📧 Observer

Observer is a server which records and archives SMTP traffic. Uses the lovely [SmtpServer](https://github.com/cosullivan/SmtpServer) project.

### Features
 - **MIME Parsing** from MimeKit deserializes Multipart data and attachments for your convenience
 - **Lua Configuration** for specific declaration of resources (and soon, hooking Observer itself!)

### Databases
 - **RavenDB**, a powerful NoSQL database written in C# with a cloud-hosting service

## Configuration



| ℹ️ | Observer requires all TLS keys to be in password-encrypted format. You can use OpenSSL for this if your certs are not password-locked.  |
| --- |:----------------------------------------------------------------------------------------------------------------------------------------|

Observer is configured with a `config.lua` file in the current working directory.

```lua
--  ⚠️ Warning!
--  All Observer configuration functions require the use of ":".
--  Using "." will produce errors!
Config:SetName("Cool Server")
Config:SetCertificate("./cert.pfx", "password")

--  Requires a TLS certificate.
--  **requires** all clients to upgrade their connection.
Config:AddSecurePort(465)
--  When a certificate is added, Unsecure ports will allow TLS connections,
--  but **will not require it**.
Config:AddUnsecurePort(587)
```

Database drivers can be individually enabled with `:Enable()`.
```lua
--  Enables the RavenDB backend
RavenDB:Enable()
--  Sets the path prefix for all emails,
--  The collection itself is locked at 'Emails'.
RavenDB:SetCollection("Emails")
--  ⚠️ **emails will not be stored if database points to a nonexistant database**
RavenDB:SetDatabase("Database")
--  Database URL
RavenDB:AddUrl("https://ravendb.cloud")
--  Certificate for authentication, if any.
--  Password required.
RavenDB:SetCertificate("client.certificate.with.password.pfx", "password")
```