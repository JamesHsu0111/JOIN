﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace MVC_Project.Models;

public partial class OfficialPhoto
{
    public int OfficialPhotoID { get; set; }

    public int? ActivityID { get; set; }

    public string PhotoPath { get; set; }

    public virtual MyActivity Activity { get; set; }
}