USE classyschedule;
-- This script will create the section table
CREATE TABLE section(
    section_id INTEGER AUTO_INCREMENT,
    section_num INTEGER,
    room_id INTEGER,
    plan_id INTEGER,
    class_id INTEGER,
    professor_id INTEGER,
    section_time_slot_id INTEGER,
    PRIMARY KEY(section_id),
    FOREIGN KEY(room_id) REFERENCES room(room_id) ON DELETE CASCADE,
    FOREIGN KEY(professor_id) REFERENCES professor(professor_id) ON DELETE CASCADE,
    FOREIGN KEY(class_id) REFERENCES class(class_id) ON DELETE CASCADE,
    FOREIGN KEY(plan_id) references plan(plan_id) ON DELETE CASCADE,
    FOREIGN KEY(section_time_slot_id) REFERENCES section_time_slot(section_time_slot_id) ON DELETE CASCADE
);
