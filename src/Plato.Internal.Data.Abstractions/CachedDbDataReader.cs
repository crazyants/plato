using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Collections;
using System.Data;

namespace Plato.Internal.Data.Abstractions
{

    public class CachedDbDataReader : DbDataReader
    {

        // TODO: Do we need to use a ConcurrentDictionary here?
        private readonly IDictionary<string, int> _ordinalMappings =
            new Dictionary<string, int>();

        private bool? _hasRows;

        IDataReader _proxy;

        public CachedDbDataReader(IDataReader proxy)
        {
            _proxy = proxy;
        }

        public override object this[int i] => _proxy[i];

        public override object this[string name] => this[GetOrdinal(name)];
        
        public override int Depth => _proxy.Depth;

        public override int FieldCount => _proxy.FieldCount;
        
        public override bool HasRows
        {
            get
            {
                if (!_hasRows.HasValue)
                    _hasRows = Read();
                return _hasRows.Value;
            }
        }

        public override bool IsClosed => _proxy.IsClosed;

        public override int RecordsAffected => _proxy.RecordsAffected;

        public override bool GetBoolean(int i) => _proxy.GetBoolean(i);

        public override byte GetByte(int i) => _proxy.GetByte(i);
      
        public override long GetBytes(int i, long dataOffset, byte[] buffer, int bufferOffset, int length) 
            => _proxy.GetBytes(i, dataOffset, buffer, bufferOffset, length);
    
        public override char GetChar(int i) => _proxy.GetChar(i);

        public override long GetChars(int i, long dataOffset, char[] buffer, int bufferOffset, int length)
            => _proxy.GetChars(i, dataOffset, buffer, bufferOffset, length);
        
        public override string GetDataTypeName(int i) => _proxy.GetDataTypeName(i);
    
        public override DateTime GetDateTime(int i) => _proxy.GetDateTime(i);
     
        public override decimal GetDecimal(int i) => _proxy.GetDecimal(i);
     
        public override double GetDouble(int i) => _proxy.GetDouble(i);

        public override IEnumerator GetEnumerator()
        {
            return new DbEnumerator(this, true);
        }

        public override Type GetFieldType(int i) => _proxy.GetFieldType(i);

        public override float GetFloat(int i) => _proxy.GetFloat(i);

        public override Guid GetGuid(int i) => _proxy.GetGuid(i);

        public override short GetInt16(int i) => _proxy.GetInt16(i);

        public override int GetInt32(int i) => _proxy.GetInt32(i);

        public override long GetInt64(int i) => _proxy.GetInt64(i);

        public override string GetName(int i) => _proxy.GetName(i);
     
        public override int GetOrdinal(string name)
        {
            if (!_ordinalMappings.TryGetValue(name, out int ordinal))
            {
                ordinal = _proxy.GetOrdinal(name);
                _ordinalMappings[name] = ordinal;
            }
            return ordinal;
        }

        public override string GetString(int i) => _proxy.GetString(i);

        public override object GetValue(int i) => _proxy.GetValue(i);
       
        public override int GetValues(object[] values) => _proxy.GetValues(values);
       
        public override bool IsDBNull(int i) => _proxy.IsDBNull(i);
       
        public override bool NextResult()
        {
            _hasRows = null;
            _ordinalMappings.Clear();
            return _proxy.NextResult();
        }

        public override bool Read()
        {
            bool result;
            if (_hasRows.HasValue)
            {
                result = _hasRows.Value;
                _hasRows = null;
            }
            else
            {
                result = _proxy.Read();
            }
            return result;
        }
    }
}
