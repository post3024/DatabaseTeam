USE classyschedule;
CREATE TABLE users (
	user_id INT NOT NULL AUTO_INCREMENT,
    user_email VARCHAR(50) NOT NULL UNIQUE,
    user_password VARCHAR(256) NOT NULL,
    salt VARCHAR(128) NOT NULL,
    user_role VARCHAR(20),
    PRIMARY KEY (user_id)
);
