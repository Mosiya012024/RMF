import * as React from 'react';
import Typography from '@mui/material/Typography';
import Breadcrumbs from '@mui/material/Breadcrumbs';
import Link from '@mui/material/Link';
import { PropaneSharp } from '@mui/icons-material';
import { useHistory } from 'react-router-dom/cjs/react-router-dom.min';

function handleClick(event) {
  event.preventDefault();
  console.info('You clicked a breadcrumb.');
}

export default function DCCBreadcrumbs(props) {
    const history = useHistory();
    const handleClickfirstPart = () => {
        history.push(props.parts[1]);
    }
    
  return (
    <div onClick={handleClick} style={{marginLeft:'50px'}}>
      <Breadcrumbs aria-label="breadcrumb">
        <Link underline="hover" color="inherit" onClick={handleClickfirstPart}>
          {props.parts[0]}
        </Link>
        {/* <Link
          underline="hover"
          color="inherit"
          href="/material-ui/getting-started/installation/"
        >
          Core
        </Link> */}
        <Typography sx={{ color: '#996bbb' }}>{props.parts[2]}</Typography>
        <Typography sx={{ color: '#996bbb' }}>{props.parts[3]}</Typography>
      </Breadcrumbs>
    </div>
  );
}
