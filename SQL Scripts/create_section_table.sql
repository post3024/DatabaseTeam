CREATE TABLE section(
  schedule_id INTEGER AUTO_INCREMENT PRIMARY KEY,
  section_number INTEGER,
  room_id INTEGER,
  plan_id INTEGER,
  class_id INTEGER,
  professor_id INTEGER,
  section_time_slot_id INTEGER,
  FOREIGN KEY(room_id) REFERENCES room(room_id),
  FOREIGN KEY(professor_id) REFERENCES professor(professor_id),
  FOREIGN KEY(class_id) REFERENCES class(class_id),
  FOREIGN KEY(plan_id) references plan(plan_id),
  FOREIGN KEY(section_time_slot_id) REFERENCES section_time_slot(section_time_slot_id)
);
