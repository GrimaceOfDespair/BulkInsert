﻿#region Copyright Notice
// This file is part of Grimace.BulkInsert.
// Bulk Insert into SQL without files
// Copyright (C) 2013 Grimace of Despair
// 
// Grimace.BulkInsert is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// Grimace.BulkInsert is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
#endregion

using System;
using System.Runtime.Serialization;

namespace Grimace.BulkInsert.Exceptions
{
  public class ColumnMismatchException : Exception
  {
    public ColumnMismatchException(string message) : base(message)
    {
    }

    public ColumnMismatchException(string message, Exception innerException) : base(message, innerException)
    {
    }

    protected ColumnMismatchException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
  }
}