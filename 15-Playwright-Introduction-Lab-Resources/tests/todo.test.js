const {test, expect} = require ('@playwright/test') 

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

//verify that user can delete task

test ("user can delete task", async ({page})=>{
    
    //arrange
    await page.goto('http://localhost:8080')
    await page.fill('#task-input', 'Test Task')
    await page.click('#add-task')

    //act 
    await page.click('.task .delete-task')

    //assert 
    //take all <li> elements
    const tasks=await page.$$eval('.tasks', tasks=>tasks.map (
        task=> task.textContent
    )) 
    expect(tasks).not.toContain("Test Task") 
})

    //verify that a user can mark a task as complete 

    test ("user can mark a task as complete", async ({page})=>{
        
        //arrange
        await page.goto('http://localhost:8080')
        await page.fill('#task-input', 'Test Task')
        await page.click('#add-task') 

        //act
        await page.click('.task .task-complete' ) 

        //assert
        const completedTask= await page.$('.task.completed')
        expect(completedTask).not.toBeNull()  
    }) 

    //verify that a user can filter tasks 

    test("user can filter tasks", async ({page})=>{
        //Arrange
        await page.goto('http://localhost:8080')
        await page.fill('#task-input', 'Test Task')
        await page.click('#add-task')
        await page.click('.task .task-complete')

        //act
        await page.selectOption('#filter', 'Completed') 

        //assert
        let incompletedTask=await page.$('.task:not(.completed)')
        expect(incompletedTask).toBeNull()
    })
