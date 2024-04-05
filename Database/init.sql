CREATE TABLE accounts (
    id INT UNSIGNED AUTO_INCREMENT PRIMARY KEY,
	nameidentifier VARCHAR(20) UNIQUE,
	name VARCHAR(255) NOT NULL,
	ms_email VARCHAR(255) NOT NULL
);

CREATE TABLE students (
	id INT UNSIGNED AUTO_INCREMENT PRIMARY KEY,
	account_id INT UNSIGNED NOT NULL UNIQUE,
    FOREIGN KEY (account_id) REFERENCES accounts(id)
);

CREATE TABLE coordinators (
	id INT UNSIGNED AUTO_INCREMENT PRIMARY KEY,
	account_id INT UNSIGNED NOT NULL UNIQUE,
    FOREIGN KEY (account_id) REFERENCES accounts(id)
);

CREATE TABLE departments (
	id INT UNSIGNED AUTO_INCREMENT PRIMARY KEY,
	name VARCHAR(255) NOT NULL,
);

CREATE TABLE semesters (
	id INT UNSIGNED AUTO_INCREMENT PRIMARY KEY,
	name VARCHAR(255) NOT NULL,
);

CREATE TABLE exams (
	id INT UNSIGNED AUTO_INCREMENT PRIMARY KEY,
	name VARCHAR(255) NOT NULL,
	batch VARCHAR(10),
    semester_id INT UNSIGNED,
	is_confirmed INT NOT NULL DEFAULT 0,
    end_date DATE,
    FOREIGN KEY (semester_id) REFERENCES semesters(id)
);

CREATE TABLE payments (
	id INT UNSIGNED AUTO_INCREMENT PRIMARY KEY,
    student_id INT UNSIGNED NOT NULL,
    exam_id INT UNSIGNED NOT NULL,
	is_verified INT UNSIGNED NOT NULL DEFAULT 0,
	receipt VARCHAR(255) NOT NULL,
    FOREIGN KEY (student_id) REFERENCES students(id),
    FOREIGN KEY (exam_id) REFERENCES exams(id),
    UNIQUE (student_id, exam_id)
);

CREATE TABLE faces {
    student_id INT UNSIGNED NOT NULL,
    exam_id INT UNSIGNED NOT NULL,
    is_proper INT UNSIGNED NOT NULL,
    FOREIGN KEY (student_id) REFERENCES students(id),
    FOREIGN KEY (exam_id) REFERENCES exams(id),
    UNIQUE (student_id, exam_id)
};

CREATE TABLE courses (
	id INT UNSIGNED AUTO_INCREMENT PRIMARY KEY,
	name VARCHAR(255) NOT NULL,
	code VARCHAR(255) NOT NULL UNIQUE
);

CREATE TABLE course_semesters (
    course_id INT UNSIGNED NOT NULL,
    semester_id INT UNSGINED NOT NULL,
    PRIMARY KEY (course_id, semester_id),
    FOREIGN KEY (semester_id) REFERENCES semesters(id),
    FOREIGN KEY (course_id) REFERENCES courses(id)
);

CREATE TABLE course_departments (
    course_id INT UNSIGNED NOT NULL,
    department_id INT UNSGINED NOT NULL,
    PRIMARY KEY (course_id, department_id),
    FOREIGN KEY (course_id) REFERENCES courses(id),
    FOREIGN KEY (department_id) REFERENCES departments(id)
);

CREATE TABLE courses_in_exam (
    id INT UNSIGNED AUTO_INCREMENT PRIMARY KEY,
    course_id INT UNSIGNED NOT NULL,
    exam_id INT UNSIGNED NOT NULL,
    department_id INT UNSGINED NOT NULL,
    coordinator_id INT UNSGINED NOT NULL,
    FOREIGN KEY (course_id) REFERENCES courses(id),
    FOREIGN KEY (exam_id) REFERENCES exams(id),
    FOREIGN KEY (department_id) REFERENCES departments(id),
    FOREIGN KEY (coordinator_id) REFERENCES coordinators(id),
    UNIQUE (exam_id, course_id, department_id)
);

CREATE TABLE approvals (
	student_id INT UNSIGNED NOT NULL,
	exam_course_id INT UNSIGNED NOT NULL,
	is_approved INT UNSIGNED NOT NULL,
    PRIMARY KEY (student_id, exam_course_id),
    FOREIGN KEY (student_id) REFERENCES students(id),
    FOREIGN KEY (exam_course_id) REFERENCES courses_in_exam(id),
    FOREIGN KEY (coordinator_id) REFERENCES coordinators(id)
);