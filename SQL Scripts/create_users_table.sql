USE classyschedule;
CREATE TABLE users (
	user_id INT NOT NULL AUTO_INCREMENT,
    user_email VARCHAR(50) NOT NULL UNIQUE,
    user_password VARCHAR(256) NOT NULL,
    salt VARCHAR(128) NOT NULL,
    PRIMARY KEY (user_id)
);
