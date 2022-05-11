USE classyschedule;
CREATE TABLE professor (
	professor_id INT AUTO_INCREMENT PRIMARY KEY,
	professor_first_name VARCHAR(40),
	professor_last_name VARCHAR(40),
	teach_load INT,
	user_email VARCHAR(50) NOT NULL UNIQUE,
    user_password VARCHAR(256) NOT NULL,
    salt VARCHAR(128) NOT NULL,
    user_role VARCHAR(20)
);