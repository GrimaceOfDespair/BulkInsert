#region Copyright Notice
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

namespace Grimace.BulkInsert.FormatFile
{
  public partial class CharTerm
  {
    public bool Equals(CharTerm other)
    {
      if (ReferenceEquals(null, other)) return false;
      if (ReferenceEquals(this, other)) return true;
      return Equals(other.tERMINATORField, tERMINATORField) && Equals(other.mAX_LENGTHField, mAX_LENGTHField) && Equals(other.cOLLATIONField, cOLLATIONField);
    }

    public override bool Equals(object obj)
    {
      if (ReferenceEquals(null, obj)) return false;
      if (ReferenceEquals(this, obj)) return true;
      if (obj.GetType() != typeof (CharTerm)) return false;
      return Equals((CharTerm) obj);
    }

    public override int GetHashCode()
    {
      unchecked
      {
        int result = (tERMINATORField != null ? tERMINATORField.GetHashCode() : 0);
        result = (result*397) ^ (mAX_LENGTHField != null ? mAX_LENGTHField.GetHashCode() : 0);
        result = (result*397) ^ (cOLLATIONField != null ? cOLLATIONField.GetHashCode() : 0);
        return result;
      }
    }
  }
}