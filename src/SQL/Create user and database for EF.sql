--MIGRATOR--------------------------------------------------------------------------
--Run this under postgres user 
--List of users:
select rolname, rolsuper, rolcreatedb, rolcanlogin from pg_roles order by rolname;
--Migrator
create user dealassiatant_migrator  with password 'dealassiatant_migrator';
alter user dealassiatant_migrator createdb;

--log in onder dealassiatant_migrator you just created
create database dealassistantdatabase owner dealassiatant_migrator;
--You might not need this
--grant all privileges on database dealassistantdatabase to dealassiatant_migrator;
--use this simple table creation scripts to make sure
create table test(id varchar(256));
insert into test values('2235dcv');
select * from test;
--drop table test;
create table test2(id varchar(256));

--APPLICATION USER
--Run this under postgres user 
--Create user
create user dealassiatantuser with password 'dealassiatantuser';
--Grant minimal rights
grant connect on database dealassistantdatabase to dealassiatantuser;
grant usage on schema public to dealassiatantuser;
--Grant CRUD for existing tables
grant select, insert, update, delete on all tables in schema public to dealassiatantuser;
--Grant CRUD for future tables
alter default privileges for user dealassiatant_migrator 
grant select, insert, update, delete on tables to dealassiatantuser;

--Test if you can connect and do all crud operation under this user

insert into test values('dealassiatantuser_data');
select * from test;
update test set id='dealassiatantuser_data_updated' where id = 'dealassiatantuser_data'
select * from test;
delete from test whereid='dealassiatantuser_data_updated'
select * from test;
