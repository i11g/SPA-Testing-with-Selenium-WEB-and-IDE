const {test, expect} = require('@playwright/test') 

//verify that user can add task 

test("user can add task", async ({page})=>{ 

    //arrange 
    await page.goto('http://localhost:8080')

    //act 
    await page.fill('#task-input', 'Test Task')
    await page.click("#add-task")

    //assert 
    const taskText= await page.textContent('.task')
    expect(taskText).toContain('Test Task') 
}) 

test ("user can dlete task", async ({page})=>{
    
    //arrange
    await page.goto('http://localhost:8080')
    await page.fill('#task-input', 'Test Task')
    await page.click('#add-task')

    //act 
    await page.click('.task .delete-task')

    //assert 
    const tasks=await page.$$eval('.tasks', tasks=>tasks.map (
        task=> task.textContent
    )) 
    expect(tasks).not.toContain("Test Task")

})