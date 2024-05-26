class StudentHome
{
    private int studentId;
    //sample implementation of student class

    //method to retrieve data from sql query
    public StudentHome(int stuentId)
    {
        this.studentId = stuentId;
        initializer();
    }
    void initializer()
    {
        //load from swl and save it into specific variables
    }

    void setAttempt(int attempt)
    {

    }
}

//needed varibles and methods in student registration class
//first all the exams must be retrieved into student page then student can filter and see the needed exam
//registration page
//for the registration page the specified courses should be offered by the department and the semester specified by the user
//the student must fill the attempt and save it to the db
