using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Constants;

public static class Policies
{
    public const string ManageCities = "ManageCities";           // only Admin
    public const string ManageProperties = "ManageProperties";   // authenticated user
}
