using System;
using System.Collections.Generic;
using System.Text;

namespace UberSy.Infrastructure.Settings;
public sealed class EmailSettings
{
    public string Host { get; set; } = null!;
    public int Port { get; set; } = 587;
    public string FromName { get; set; } = null!;
    public string FromAddress { get; set; } = null!;
    public string Username { get; set; } = null!;
    public string Password { get; set; } = null!;
}
