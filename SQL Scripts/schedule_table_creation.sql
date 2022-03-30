use classyschedule;
create table schedule(
schedule_id integer auto_increment primary key,
section_number integer,
class_num integer,
dept_id integer,
room_id integer,
professor_id integer,
foreign key(room_id) references room(room_id),
foreign key(professor_id) references professor(professor_id),
foreign key(class_num) references class(class_num),
foreign key(dept_id) references department(dept_id)
)