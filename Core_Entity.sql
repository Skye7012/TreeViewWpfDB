create database Core_Entity
use Core_Entity


create table TGROUP(
	id int primary key identity not null,
	name varchar(255) not null);

insert TGROUP
values ('Первая группа');
insert TGROUP
values ('Вторая группа');
insert TGROUP
values ('Третья группа');
insert TGROUP
values ('Четвертая  группа');
insert TGROUP
values ('Пятая  группа');


create table TRELATION(
	id_parent int not null references TGROUP(Id),
	id_child int not null references TGROUP(Id),
	constraint TRELATION_num primary key (id_parent,id_child));

insert TRELATION
values (1, 2);
insert TRELATION
values (1, 3);
insert TRELATION
values (2, 4);


create table TPROPERTY(
	id int primary key identity not null,
	name varchar(255) not null,
	value varchar(255) not null,
	group_id int not null references TGROUP(Id));

insert TPROPERTY
values ('Свойство 1', 'Описание свойства 1', 1);
insert TPROPERTY
values ('Свойство 2', 'Описание свойства 2', 3);
