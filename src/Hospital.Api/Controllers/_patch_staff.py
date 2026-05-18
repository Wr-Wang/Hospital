import re

with open('StaffController.cs', 'r', encoding='utf-8') as f:
    content = f.read()

# Replace method-level attributes
content = content.replace(
    '    [HttpPost]\n    public async Task<IActionResult> Create([FromBody] CreateStaffRequest request)',
    '    /// <summary>新建人员</summary>\n    [HttpPost]\n    public async Task<IActionResult> Create([FromBody] CreateStaffRequest request)'
)
content = content.replace(
    '    [HttpPut("{id:long}")]\n    public async Task<IActionResult> Update(long id, [FromBody] UpdateStaffRequest request)',
    '    /// <summary>更新人员信息</summary>\n    [HttpPut("{id:long}")]\n    public async Task<IActionResult> Update(long id, [FromBody] UpdateStaffRequest request)'
)
content = content.replace(
    '    [HttpPatch("{id:long}/license")]\n    public async Task<IActionResult> UpdateLicense(long id, [FromBody] UpdateStaffLicenseRequest request)',
    '    /// <summary>更新人员执业资质</summary>\n    [HttpPatch("{id:long}/license")]\n    public async Task<IActionResult> UpdateLicense(long id, [FromBody] UpdateStaffLicenseRequest request)'
)
content = content.replace(
    '    [HttpPatch("{id:long}/activate")]\n    public async Task<IActionResult> Activate(long id)',
    '    /// <summary>启用人员</summary>\n    [HttpPatch("{id:long}/activate")]\n    public async Task<IActionResult> Activate(long id)'
)
content = content.replace(
    '    [HttpPatch("{id:long}/deactivate")]\n    public async Task<IActionResult> Deactivate(long id)',
    '    /// <summary>停用人员</summary>\n    [HttpPatch("{id:long}/deactivate")]\n    public async Task<IActionResult> Deactivate(long id)'
)

with open('StaffController.cs', 'w', encoding='utf-8') as f:
    f.write(content)
print("StaffController.cs patched successfully")
