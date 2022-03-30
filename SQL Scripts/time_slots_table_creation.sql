create table time_slots(
time_slot_id integer auto_increment primary key,
start_time time,
end_time time,
day_of_week varchar(9)
)