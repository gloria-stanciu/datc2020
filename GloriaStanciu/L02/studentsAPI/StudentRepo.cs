using System.Collections.Generic;

namespace studentsAPI
{
    public static class StudentRepo
    {
        private static List<Student> students = new List<Student>();

        public static void insert(Student data) {
            data.Id = students.Count;
            students.Add(data);
        }

        public static Student getById(int id) {
            return students.Find(student => student.Id == id);
        }

        public static List<Student> getAll() {
            return students;
        }
        
        public static void deleteById(int id) {
            students.RemoveAll(s => s.Id == id);
        }
        
        public static Student update(int id, Student student){
            int index = students.FindIndex(s => s.Id == id);
            students[index].FirstName = student.FirstName;
            students[index].LastName = student.LastName;
            students[index].Faculty = student.Faculty;
            students[index].YearOfStudy = student.YearOfStudy;
            return students[index];
        }
    }
}
