create table preference(
preference_id integer auto_increment primary key,
professor_id integer,
time_slot_id integer,
preference integer,
foreign key(professor_id) references professor(professor_id) on delete cascade,
foreign key(time_slot_id) references time_slots(time_slot_id) on delete cascade
)