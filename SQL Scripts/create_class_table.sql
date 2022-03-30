USE classyschedule;
CREATE TABLE class (
	class_num INT NOT NULL,
    dept_id INT NOT NULL,
    class_name VARCHAR(40) NULL,
    capacity INT NULL,
    credits INT NULL,
    PRIMARY KEY (class_num, dept_id),
    FOREIGN KEY (dept_id)
		REFERENCES department(dept_id)
        ON DELETE CASCADE
);