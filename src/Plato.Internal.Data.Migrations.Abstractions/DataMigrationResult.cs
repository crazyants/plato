using System;
using System.Collections.Generic;

namespace Plato.Internal.Data.Migrations.Abstractions
{
    public class DataMigrationResult
    {

        private List<Exception> _errors;
        private List<DataMigration> _successfulMigrations;
        private List<DataMigration> _failedMigrations;

        public List<Exception> Errors
        {
            get => _errors ?? (_errors = new List<Exception>());
            set => _errors = value;
        }

        public List<DataMigration> SuccessfulMigrations
        {
            get => _successfulMigrations ?? (_successfulMigrations = new List<DataMigration>());
            set => _successfulMigrations = value;
        }

        public List<DataMigration> FailedMigrations
        {
            get => _failedMigrations ?? (_failedMigrations = new List<DataMigration>());
            set => _failedMigrations = value;
        }

    }
}
