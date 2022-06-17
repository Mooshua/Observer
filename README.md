# 📧 Observer

Observer is a server which records and archives SMTP traffic. Uses the lovely [SmtpServer](https://github.com/cosullivan/SmtpServer) project.

### Features
 - **MIME Parsing** from MimeKit deserializes Multipart data and attachments for your convenience
 - **Lua Configuration** for specific declaration of resources, and to provide custom implementations for filter functions.

### Databases
 - **RavenDB**, a powerful NoSQL database written in C# with a cloud-hosting service. Raw, encoded message binaries are always included as an attachment.

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

Hooks can be used to modify the behavior of observer based on your own logic. Currently, four hooks are provided.
```lua
Hooks.OnTransmit = function(from, size)
    --  Called before the message is processed.
    --  Includes size and from address to decide whether or not to accept or discard it.

    --  return FilterCode.Yes
    --  return FilterCode.NoTemporarily
    --  return FilterCode.NoPermanently
    --  return FilterCode.SizeLimitExceeded
end

Hooks.OnReceive = function(from, to)
    --  Called before the message is stored.
    --  Allows you to chuck out and block

    --  Returns same as OnTransmit
end

Hooks.OnStore = function(email, from, to)
    --  Determines whether or not to **store the email.**
    --  The sender will still receive a status code and believe the message has been delivered.
    --  "email" is the same class as "ModelMessage", in server/emails.cs.
    
    --  Returns a bool. 
end

Hooks.OnRespond = function(email, from, to)
    --  Determines the response code for this email.
    --  Requires return of { code = CODE, message = REASON }.
    
    --  Response codes are an enum in the Lua global ResponseCode.
    --  Examples:
    --  ResponseCode.Ok
    --  ResponseCode.ConnectionRefused
    --  ResponseCode.ServiceUnavailable
    --  ResponseCode.Overloaded
    --  ResponseCode.Unavailable
    
    --  All others are in enum SmtpReplyCode in SmtpServer.

    return { code = ResponseCode.Ok, message = "Cool beans" }
end
```