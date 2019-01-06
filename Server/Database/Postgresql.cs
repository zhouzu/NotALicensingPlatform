﻿using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace Server.Database
{
    class Postgresql : IDBProvider
    {
        private static string ConnectionString;

        private readonly UserContext db;

        public Postgresql(string connectionString)
        {
            ConnectionString = connectionString;

            db = new UserContext();

            if (!db.Database.CanConnect())
                throw new Exception($"Failed to connect to database using the connection string: {connectionString}");
        }

        public bool CreateKey(Key key)
        {
            db.Keys.Add(key);

            try
            {
                db.SaveChanges();

                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool CreateUser(User user)
        {
            db.Users.Add(user);

            try
            {
                db.SaveChanges();

                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool DeleteKey(Key key)
        {
            db.Keys.Remove(key);

            try
            {
                db.SaveChanges();

                return true;
            }
            catch
            {
                return false;
            }
        }

        public Key GetKey(string id)
        {
            return db.Keys.Find(id);
        }

        public User GetUser(string username)
        {
            var user = db.Users.Find(username);

            if (user != null)
                db.Entry(user).Collection(x => x.Keys).Load();

            return user;
        }

        public bool UpdateUser(User user)
        {
            db.Users.Update(user);

            try
            {
                db.SaveChanges();

                return true;
            }
            catch
            {
                return false;
            }
        }

        internal class UserContext : DbContext
        {
            public DbSet<User> Users { get; set; }

            public DbSet<Key> Keys { get; set; }

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            {
                optionsBuilder.UseNpgsql(ConnectionString);
            }
        }
    }
}
