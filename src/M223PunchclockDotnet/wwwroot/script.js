const URL = '';
let entries = [];

const dateAndTimeToDate = (dateString, timeString) => {
    return new Date(`${dateString}T${timeString}`).toISOString();
};

const createEntry = (e) => {
    e.preventDefault();
    const formData = new FormData(e.target);
    const entry = {};
    entry['checkIn'] = dateAndTimeToDate(formData.get('checkInDate'), formData.get('checkInTime'));
    entry['checkOut'] = dateAndTimeToDate(formData.get('checkOutDate'), formData.get('checkOutTime'));

    fetch(`${URL}/entries`, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(entry)
    }).then((result) => {
        result.json().then((entry) => {
            entries.push(entry);
            renderEntries();
        });
    });
};

const deleteEntry = async (id) => {
    const response = await fetch(`${URL}/entries/${id}`, {
        method: 'DELETE'
    });

    if (response.ok) {
        entries = entries.filter(entry => entry.id !== id);
        renderEntries();
    }
};

const editEntry = async (entry) => {
    const checkIn = prompt(
        'Check-in (Format: YYYY-MM-DDTHH:mm)',
        new Date(entry.checkIn).toISOString().slice(0, 16)
    );

    if (checkIn === null) return;

    const checkOut = prompt(
        'Check-out (Format: YYYY-MM-DDTHH:mm)',
        new Date(entry.checkOut).toISOString().slice(0, 16)
    );

    if (checkOut === null) return;

    const updatedEntry = {
        checkIn: new Date(checkIn).toISOString(),
        checkOut: new Date(checkOut).toISOString()
    };

    const response = await fetch(`${URL}/entries/${entry.id}`, {
        method: 'PUT',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(updatedEntry)
    });

    if (response.ok) {
        entry.checkIn = updatedEntry.checkIn;
        entry.checkOut = updatedEntry.checkOut;
        renderEntries();
    }
};

const indexEntries = () => {
    fetch(`${URL}/entries`, {
        method: 'GET'
    }).then((result) => {
        result.json().then((result) => {
            entries = result;
            renderEntries();
        });
    });
    renderEntries();
};

const createCell = (text) => {
    const cell = document.createElement('td');
    cell.innerText = text;
    return cell;
};

const renderEntries = () => {
    const display = document.querySelector('#entryDisplay');
    display.innerHTML = '';
    entries.forEach((entry) => {
        const row = document.createElement('tr');
        row.appendChild(createCell(entry.id));
        row.appendChild(createCell(new Date(entry.checkIn).toLocaleString()));
        row.appendChild(createCell(new Date(entry.checkOut).toLocaleString()));
 
        const deleteButton = document.createElement('button');
        deleteButton.innerText = 'Delete';
        deleteButton.addEventListener('click', () => deleteEntry(entry.id));

        const editButton = document.createElement('button');
        editButton.innerText = 'Edit';
        editButton.addEventListener('click', () => editEntry(entry));

        const deleteCell = document.createElement('td');
        deleteCell.appendChild(deleteButton);
        row.appendChild(deleteCell);

        const editCell = document.createElement('td');
        editCell.appendChild(editButton);
        row.appendChild(editCell);
        display.appendChild(row);
    });
};

document.addEventListener('DOMContentLoaded', function(){
    const createEntryForm = document.querySelector('#createEntryForm');
    createEntryForm.addEventListener('submit', createEntry);
    indexEntries();
});