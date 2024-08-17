﻿using NpWebApiClientApp;
using System.Net.Http.Json;

HttpClient client = new();

List<Employee>? employees;

Employee? employee = null;

Error? error = null;

HttpContent content = null;

int id;
string name;
int age;

string text;

string url = "https://localhost:7093/";


while(true)
{
    Console.WriteLine("1 - List employees");
    Console.WriteLine("2 - View employee");
    Console.WriteLine("3 - Add employee");
    Console.WriteLine("4 - Edit employee");
    Console.WriteLine("5 - Delete employee\n");

    Console.WriteLine("6 - Send Form");
    Console.WriteLine("7 - Send Stream");
    Console.WriteLine("8 - Send Bytes");
    Console.WriteLine("9 - Send Big Form\n");

    Console.WriteLine("0 - Quit");

    Console.Write("Your select: ");
    int select = Int32.Parse(Console.ReadLine());

    if (select == 0) break;

    switch(select)
    {
        case 1:
            employees = await client.GetFromJsonAsync<List<Employee>>(url + "api/empl");
            if(employees is not null)
            {
                Console.WriteLine("\tEmployees's list:");
                foreach(var e in employees)
                    Console.WriteLine($"\t\t{e}");
            }
            break;
        case 2:
            Console.Write("Input ID of employee: ");
            id = Int32.Parse(Console.ReadLine());

            using (var response = await client.GetAsync($"{url}api/empl/{id}"))
            {
                if(response.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    error = await response.Content.ReadFromJsonAsync<Error>();
                    Console.WriteLine($"\t{error?.Message}");
                }
                else
                {
                    employee = await response.Content.ReadFromJsonAsync<Employee>();
                    Console.WriteLine($"\t{employee}");
                }
            }
            break;
        case 3:
            Console.Write("\tInput name: ");
            name = Console.ReadLine()!;
            Console.Write("\tInput age: ");
            age = Int32.Parse(Console.ReadLine()!);

            employee = new(name, age);

            using (var response = await client.PostAsJsonAsync($"{url}api/empl/", employee))
            {
                employee = await response.Content.ReadFromJsonAsync<Employee>();
                Console.WriteLine($"\tEmplyee add: {employee}");
            }
            break;
        case 4:
            Console.Write("Input ID of employee for edit: ");
            id = Int32.Parse(Console.ReadLine());

            using (var response = await client.GetAsync($"{url}api/empl/{id}"))
            {
                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                    Console.WriteLine("\tEmployee not found");
                else
                {
                    employee = await response.Content.ReadFromJsonAsync<Employee>();
                }
            }

            Console.Write($"\tInput name <{employee.Name}>: ");
            name = Console.ReadLine()!;
            Console.Write($"\tInput age <{employee.Age}>: ");
            age = Int32.Parse(Console.ReadLine()!);
            employee.Name = name;
            employee.Age = age;

            using (var response = await client.PutAsJsonAsync($"{url}api/empl", employee))
            {
                if(response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    employee = await response.Content.ReadFromJsonAsync<Employee>();
                    Console.WriteLine($"\tEmployee edited: {employee}");
                }
                else
                    Console.WriteLine("\tError");

            }
            break;
        case 5:
            Console.Write("Input ID of employee for delete: ");
            id = Int32.Parse(Console.ReadLine());

            using(var response = await client.DeleteAsync($"{url}api/empl/{id}"))
            {
                if(response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    employee = await response.Content.ReadFromJsonAsync<Employee>();
                    Console.WriteLine($"\tEmployee deleted: {employee}");
                }
                else
                    Console.WriteLine("\tError");
            }

            break;
        case 6:
            Dictionary<string, string> form = new()
            {
                ["id"] = "100",
                ["name"] = "Mikky",
                ["age"] = "40",
            };

            content = new FormUrlEncodedContent(form);

            using (var response = await client.PostAsync($"{url}form", content))
            {
                text = await response.Content.ReadAsStringAsync();
                Console.WriteLine(text);
            }
            break;
        case 7:
            string filePath = Directory.GetCurrentDirectory() + "/image.jpg";
            using(FileStream stream = File.OpenRead(filePath))
            {
                content = new StreamContent(stream);
                var response = await client.PostAsync($"{url}upload", content);
                string textResponse = await response.Content.ReadAsStringAsync();
                Console.WriteLine(textResponse);
            }
            break;

        case 8:
            text = "Lorem ipsum and Hello world!";
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(text);

            content = new ByteArrayContent(buffer);

            using (var response = await client.PostAsync($"{url}bindata", content))
            {
                text = await response.Content.ReadAsStringAsync();
                Console.WriteLine(text);
            }
            break;

        case 9:
            string filePath2 = $"{Directory.GetCurrentDirectory()}/book01.pdf";
            using(MultipartFormDataContent contentData = new())
            {
                var contentFiles = new StreamContent(File.OpenRead(filePath2));
                contentFiles.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/pdf");
                contentData.Add(contentFiles, name: "file", fileName: "mybook01.pdf");

                using(var response = await client.PostAsync($"{url}bigform", contentData))
                {
                    text = await response.Content.ReadAsStringAsync();
                    Console.WriteLine(text);
                }
            }
            break;
        default:
            break;
    }
}


class Error { public string? Message { set; get; } }