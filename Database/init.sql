CREATE TABLE accounts (
    id INT UNSIGNED AUTO_INCREMENT PRIMARY KEY,
    nameidentifier VARCHAR(20) UNIQUE,
    name VARCHAR(255) NOT NULL,
    ms_email VARCHAR(255) NOT NULL
);

CREATE TABLE students (
    id INT UNSIGNED AUTO_INCREMENT PRIMARY KEY,
    account_id INT UNSIGNED NOT NULL UNIQUE,
    FOREIGN KEY (account_id) REFERENCES accounts(id) ON DELETE RESTRICT ON UPDATE CASCADE
);

CREATE TABLE coordinators (
    id INT UNSIGNED AUTO_INCREMENT PRIMARY KEY,
    account_id INT UNSIGNED NOT NULL UNIQUE,
    FOREIGN KEY (account_id) REFERENCES accounts(id) ON DELETE RESTRICT ON UPDATE CASCADE
);

CREATE TABLE advisors (
    id INT UNSIGNED AUTO_INCREMENT PRIMARY KEY,
    account_id INT UNSIGNED NOT NULL UNIQUE,
    FOREIGN KEY (account_id) REFERENCES accounts(id) ON DELETE RESTRICT ON UPDATE CASCADE
);

CREATE TABLE departments (
    id INT UNSIGNED AUTO_INCREMENT PRIMARY KEY,
    name VARCHAR(255) NOT NULL
);

CREATE TABLE semesters (
    id INT UNSIGNED AUTO_INCREMENT PRIMARY KEY,
    name VARCHAR(255) NOT NULL
);

CREATE TABLE exams (
    id INT UNSIGNED AUTO_INCREMENT PRIMARY KEY,
    name VARCHAR(255) NOT NULL,
    batch VARCHAR(10),
    semester_id INT UNSIGNED,
    is_confirmed INT NOT NULL DEFAULT 0,
    coordinator_approval_extension INT UNSIGNED,
    advisor_approval_extension INT UNSIGNED,
    end_date DATE,
    FOREIGN KEY (semester_id) REFERENCES semesters(id) ON DELETE RESTRICT ON UPDATE CASCADE
);

-- one student can sit for for several exams and based on it the rest will change
CREATE TABLE students_in_exam (
    id INT UNSIGNED AUTO_INCREMENT PRIMARY KEY,
    student_id INT UNSIGNED NOT NULL,
    exam_id INT UNSIGNED NOT NULL,
    is_proper INT UNSIGNED NOT NULL,
    advisor_id INT UNSIGNED,
    FOREIGN KEY (student_id) REFERENCES students(id) ON DELETE RESTRICT ON UPDATE CASCADE,
    FOREIGN KEY (exam_id) REFERENCES exams(id) ON DELETE RESTRICT ON UPDATE CASCADE,
    FOREIGN KEY (advisor_id) REFERENCES advisors(id) ON DELETE RESTRICT ON UPDATE CASCADE,
    UNIQUE (student_id, exam_id)
);

-- only one student make payment per one exam once
CREATE TABLE payments (
    id INT UNSIGNED AUTO_INCREMENT PRIMARY KEY,
    student_id INT UNSIGNED NOT NULL,
    exam_id INT UNSIGNED NOT NULL,
    is_verified INT UNSIGNED NOT NULL DEFAULT 0,
    receipt VARCHAR(255) NOT NULL,
    FOREIGN KEY (student_id) REFERENCES students(id) ON DELETE RESTRICT ON UPDATE CASCADE,
    FOREIGN KEY (exam_id) REFERENCES exams(id) ON DELETE RESTRICT ON UPDATE CASCADE,
    UNIQUE (student_id, exam_id)
);

CREATE TABLE courses (
    id INT UNSIGNED AUTO_INCREMENT PRIMARY KEY,
    name VARCHAR(255) NOT NULL,
    semester_id INT UNSIGNED NOT NULL,
    code VARCHAR(255) NOT NULL UNIQUE
);

-- muliple departments can offer the same course
CREATE TABLE course_departments (
    course_id INT UNSIGNED NOT NULL,
    department_id INT UNSIGNED NOT NULL,
    PRIMARY KEY (course_id, department_id),
    FOREIGN KEY (course_id) REFERENCES courses(id) ON DELETE RESTRICT ON UPDATE CASCADE,
    FOREIGN KEY (department_id) REFERENCES departments(id) ON DELETE RESTRICT ON UPDATE CASCADE,
    UNIQUE (course_id, department_id)
);

-- instance of the actual course for each exam offered by available departments
CREATE TABLE courses_in_exam (
    id INT UNSIGNED AUTO_INCREMENT PRIMARY KEY,
    course_id INT UNSIGNED NOT NULL,
    exam_id INT UNSIGNED NOT NULL,
    department_id INT UNSIGNED NOT NULL, -- offering department
    coordinator_id INT UNSIGNED NOT NULL,
    FOREIGN KEY (course_id) REFERENCES courses(id) ON DELETE RESTRICT ON UPDATE CASCADE,
    FOREIGN KEY (exam_id) REFERENCES exams(id) ON DELETE RESTRICT ON UPDATE CASCADE,
    FOREIGN KEY (department_id) REFERENCES departments(id) ON DELETE RESTRICT ON UPDATE CASCADE,
    FOREIGN KEY (coordinator_id) REFERENCES coordinators(id) ON DELETE RESTRICT ON UPDATE CASCADE,
    UNIQUE (exam_id, course_id, department_id)
);

CREATE TABLE student_registration (
    exam_student_id INT UNSIGNED NOT NULL,
    exam_course_id INT UNSIGNED NOT NULL,
    is_approved INT UNSIGNED NOT NULL,
    PRIMARY KEY (exam_student_id, exam_course_id),
    FOREIGN KEY (exam_student_id) REFERENCES students_in_exam(id) ON DELETE RESTRICT ON UPDATE CASCADE,
    FOREIGN KEY (exam_course_id) REFERENCES courses_in_exam(id) ON DELETE RESTRICT ON UPDATE CASCADE
);

CREATE TABLE administrators (
    id INT UNSIGNED AUTO_INCREMENT PRIMARY KEY,
    account_id INT UNSIGNED NOT NULL UNIQUE,
    FOREIGN KEY (account_id) REFERENCES accounts(id) ON DELETE RESTRICT ON UPDATE CASCADE
);
