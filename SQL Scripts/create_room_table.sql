USE classyschedule;
-- This script will create the room table
CREATE TABLE room(
    room_id INTEGER AUTO_INCREMENT,
    capacity INTEGER,
    room_num INTEGER,
    building_name VARCHAR(4),
    PRIMARY KEY(room_id)
);
