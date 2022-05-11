USE classyschedule;
CREATE TABLE time_slot(
	time_slot_id INTEGER AUTO_INCREMENT,
	start_time VARCHAR(6),
	end_time VARCHAR(6),
    PRIMARY KEY(time_slot_id)
);

INSERT INTO time_slot (start_time, end_time)
	VALUES ("08:00", "09:40"),
		   ("08:15", "09:20"),
		   ("09:35", "10:40"),
           ("09:55", "11:35"),
           ("10:55", "12:00"),
           ("12:15", "13:20"),
           ("13:30", "15:10"),
           ("13:35", "14:40"),
           ("15:25", "17:00"),
           ("17:30", "19:15"),
           ("19:30", "21:15");