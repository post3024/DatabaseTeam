USE classyschedule;
CREATE TABLE class_preference (
	class_num SMALLINT NOT NULL,
    dept_id INT NOT NULL,
    is_lab BOOLEAN NOT NULL,
    prof_id INT NOT NULL,
    can_teach BOOLEAN,
    prefer_to_teach BOOLEAN,
    PRIMARY KEY (class_num, dept_id, is_lab, prof_id),
	FOREIGN KEY (class_num, dept_id, is_lab) REFERENCES class(class_num, dept_id, is_lab),
    FOREIGN KEY (prof_id) REFERENCES professor(prof_id)
);