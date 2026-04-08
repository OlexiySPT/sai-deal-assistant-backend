--log in onder dealassiatant_migrator
create database dealassistantdatabase_mocked_data owner dealassiatant_migrator;
--drop database dealassistantdatabase;
--You might not need this
--grant all privileges on database dealassistantdatabase to dealassiatant_migrator;
--use this simple table creation scripts to make sure
create table test(id varchar(256));
insert into test values('2235dcv');
select * from test;
--drop table test;
create table test2(id varchar(256));

--Grant minimal rights
grant connect on database dealassistantdatabase_mocked_data to dealassiatantuser;
grant usage on schema public to dealassiatantuser;
--Grant CRUD for existing tables
grant select, insert, update, delete on all tables in schema public to dealassiatantuser;
--Grant CRUD for future tables
alter default privileges for user dealassiatant_migrator in schema public
grant select, insert, update, delete on tables to dealassiatantuser;

--Test if you can connect and do all crud operation under this user

insert into test values('dealassiatantuser_data');
select * from test;
update test set id='dealassiatantuser_data_updated' where id = 'dealassiatantuser_data';
select * from test;
delete from test where id = 'dealassiatantuser_data_updated';
select * from test;
