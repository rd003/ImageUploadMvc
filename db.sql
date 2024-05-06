create database ImageUploadMvc

use ImageUploadMvc

create table Person
(
  Id int primary key identity,
  FirstName nvarchar(20) not null,
  LastName nvarchar(20) not null,
  ProfilePicture nvarchar(100)
)
