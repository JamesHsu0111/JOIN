﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace MVC_Project.Models;

public partial class Registration
{
    public int RegistrationID { get; set; }

    public int? GroupID { get; set; }

    public int? ParticipantID { get; set; }

    public virtual Group Group { get; set; }

    public virtual Member Participant { get; set; }
}