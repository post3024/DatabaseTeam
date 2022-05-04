alter table section_time_slot
add constraint uniqueness unique (start_time, end_time,on_monday,on_tuesday, on_wednesday, on_thursday, on_friday)