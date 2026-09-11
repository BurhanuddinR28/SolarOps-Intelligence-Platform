import csv
import json
import random
from datetime import datetime, timedelta

# Config
NUM_READINGS = 1000
PLANTS = ["Perrysburg", "Toledo"]
TOOLS = [("Boiler", "A"), ("Boiler", "B"), ("Flasher", "A"), ("Flasher", "B")]
OUTPUT_DIR = "data/raw"

# Generate random subIDs
def generate_sub_id(i):
    return f"FS-{2026}-{str(i).zfill(4)}"

# Generate random timestamp within last 30 days
def random_timestamp():
    base = datetime(2026, 8, 1)
    offset = timedelta(minutes=random.randint(0, 43200))
    return (base + offset).strftime("%Y-%m-%d %H:%M:%S")

# Generate panel readings
readings = []
for i in range(1, NUM_READINGS + 1):
    tool = random.choice(TOOLS)
    readings.append({
        "ReadingId": i,
        "SubId": generate_sub_id(i),
        "Timestamp": random_timestamp(),
        "ReadTime": round(random.uniform(0.5, 3.0), 2),
        "Wattage": round(random.uniform(200.0, 400.0), 2),
        "Plant": random.choice(PLANTS),
        "ToolName": tool[0],
        "ToolLine": tool[1],
        "PassFail": random.choice([0, 1]),
        "CreatedAt": datetime.now().strftime("%Y-%m-%d %H:%M:%S")
    })

# Write panel_readings.csv
with open(f"{OUTPUT_DIR}/panel_readings.csv", "w", newline="") as f:
    writer = csv.DictWriter(f, fieldnames=readings[0].keys())
    writer.writeheader()
    writer.writerows(readings)

print(f"Generated {len(readings)} panel readings")

# Generate IV curves (10 pairs per reading)
iv_curves = []
iv_id = 1
for r in readings:
    for v in range(10):
        voltage = round(v * 0.5, 2)
        current = round(random.uniform(7.0, 9.0) - (v * 0.1), 2)
        iv_curves.append({
            "IVId": iv_id,
            "ReadingId": r["ReadingId"],
            "Voltage": voltage,
            "CurrentAmp": current
        })
        iv_id += 1

# Write iv_curves.csv
with open(f"{OUTPUT_DIR}/iv_curves.csv", "w", newline="") as f:
    writer = csv.DictWriter(f, fieldnames=iv_curves[0].keys())
    writer.writeheader()
    writer.writerows(iv_curves)

print(f"Generated {len(iv_curves)} IV curve entries")

# Generate fault events (10% of readings)
faults = []
fault_reasons = ["Overvoltage", "Undervoltage", "Overtemperature", "Tool malfunction"]
for r in random.sample(readings, NUM_READINGS // 10):
    faults.append({
        "SubId": r["SubId"],
        "Timestamp": r["Timestamp"],
        "FaultReason": random.choice(fault_reasons),
        "ToolName": r["ToolName"],
        "ToolLine": r["ToolLine"]
    })

# Write fault_events.json
with open(f"{OUTPUT_DIR}/fault_events.json", "w") as f:
    json.dump(faults, f, indent=2)

print(f"Generated {len(faults)} fault events")